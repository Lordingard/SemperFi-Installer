"""Validate public sources without original game files."""
import hashlib
import pathlib
import struct
import subprocess
ROOT = pathlib.Path(__file__).resolve().parents[1]
files = subprocess.check_output(["git", "ls-files", "-z"], cwd=ROOT).decode().split("\0")
for name in filter(None, files):
    p = pathlib.PurePosixPath(name)
    assert p.parts[0] not in {".local", "bin", "artifacts", "SEMPERFI", "Jeu", "Jeu-Compatible", "SemperFi-Autonome"}, name
    assert p.suffix.lower() not in {".iso", ".exe", ".wav", ".avi", ".pcx", ".sav", ".dmp", ".log"}, name
    assert p.suffix.lower() != ".dll" or name == "third_party/DDrawCompat/ddraw.dll", name
    assert (ROOT / name).stat().st_size < 5_000_000, name
assert hashlib.sha256((ROOT / "third_party/DDrawCompat/ddraw.dll").read_bytes()).hexdigest().upper() == "F75F0AC48D2782F225C483DC2F1142303A513E8DD8A60793891ADE89F64755EA"
patch = (ROOT / "patches/sf-autonomous.patch").read_bytes()
assert patch[:8] == b"SFPATCH1"
original, final, count = struct.unpack_from("<III", patch, 8)
assert 0 < original <= final < 4_000_000 and 0 < count < 1000
cursor = 20
for _ in range(count):
    start, before, after = struct.unpack_from("<III", patch, cursor)
    cursor += 12
    assert start + before <= original and start + after <= final
    cursor += before + after
assert cursor == len(patch) and len(patch) < 10_000
rows = (ROOT / "src/payload.tsv").read_text(encoding="utf-8-sig").splitlines()
assert len(rows) == 261
seen = set()
for row in rows:
    name, size, checksum = row.split("\t")
    assert not pathlib.PureWindowsPath(name).is_absolute() and ".." not in pathlib.PureWindowsPath(name).parts
    assert name.lower() not in seen
    seen.add(name.lower())
    assert int(size) >= 0 and len(checksum) == 64
    int(checksum, 16)
print("Public files, dependency checksum, patch bounds and 261-file manifest: OK")
