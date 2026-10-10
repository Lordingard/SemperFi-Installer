"""Regenerate the published delta from privately held, known original/patched files."""
import argparse
import hashlib
import pathlib
import struct
p = argparse.ArgumentParser()
p.add_argument("original", type=pathlib.Path)
p.add_argument("patched", type=pathlib.Path)
p.add_argument("--output", type=pathlib.Path, required=True)
a = p.parse_args()
old = a.original.read_bytes()
new = a.patched.read_bytes()
assert hashlib.sha256(old).hexdigest().upper() == "575F85F4FFE843FF7B49CDD7C8EF3EC9993843CA60ADA16D832B7D5279C9AA26"
assert hashlib.sha256(new).hexdigest().upper() == "E46DBA94E8CECEB313465A1FE4EB824BE1FAAC864CDB6399A4DF8A7097F75EB9"
spans = []
start = last = None
for i in range(len(old)):
    if old[i] != new[i]:
        if start is None:
            start = i
        elif i - last > 16:
            spans.append((start, last + 1))
            start = i
        last = i
if start is not None:
    spans.append((start, last + 1))
if len(new) > len(old):
    spans.append((len(old), len(new)))
delta = bytearray(b"SFPATCH1") + struct.pack("<III", len(old), len(new), len(spans))
for lo, hi in spans:
    before, after = old[lo:hi], new[lo:hi]
    delta += struct.pack("<III", lo, len(before), len(after)) + before + after
a.output.write_bytes(delta)
print(f"{len(delta)} bytes; {len(spans)} spans")
