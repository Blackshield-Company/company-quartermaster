#!/usr/bin/env python3
"""Regression check: every non-templated NPC_ in the plugin must carry a body skin tone (QNAM).

Without QNAM the engine tints the body black while FaceGen keeps the face normal (0.2.1 bug).
Usage: check_npc_appearance.py Plugin.esp
"""
import struct, sys, zlib

def subs(d):
    i = 0
    while i < len(d):
        t, n = struct.unpack_from("<4sH", d, i); yield t, d[i+6:i+6+n]; i += 6 + n

def records(buf, off, end):
    while off < end:
        t = buf[off:off+4]; size = struct.unpack_from("<I", buf, off+4)[0]
        if t == b"GRUP":
            yield from records(buf, off+24, off+size); off += size
        else:
            yield t, struct.unpack_from("<I", buf, off+8)[0], buf[off+24:off+24+size]; off += 24 + size

buf = open(sys.argv[1], "rb").read()
bad = 0; n = 0
for t, flags, body in records(buf, 24 + struct.unpack_from("<I", buf, 4)[0], len(buf)):
    if t != b"NPC_": continue
    if flags & 0x40000: body = zlib.decompress(body[4:])
    s = dict(subs(body)); n += 1
    templated = b"TPLT" in s and struct.unpack_from("<H", s[b"ACBS"], 18)[0] & 0x1  # Traits
    if templated: continue
    q = s.get(b"QNAM")
    edid = s.get(b"EDID", b"?").rstrip(b"\0").decode()
    if not q or struct.unpack("<fff", q[:12]) == (0.0, 0.0, 0.0):
        print(f"FAIL {edid}: no body skin tone (QNAM)"); bad += 1
    else:
        print(f"ok   {edid}: QNAM {tuple(round(x,3) for x in struct.unpack('<fff', q[:12]))}")
print(f"{n} NPC_ checked, {bad} failing"); sys.exit(1 if bad else 0)
