#!/usr/bin/env python3
"""Regression check: every non-templated NPC_ in the plugin must carry a body skin tone (QNAM)
and a face skin-tone tint layer (TINI/TINC/TINV) whose colour matches QNAM.

No FaceGen ships, so the face tint is built at runtime from the tint layers; if it differs
from QNAM the face and body render in different tones (0.2.2 report).
Without QNAM the engine tints the body black while FaceGen keeps the face normal (0.2.1 bug).
Usage: check_npc_appearance.py Plugin.esp
"""
import struct, sys, zlib

def subs(d):
    i = 0
    while i < len(d):
        t, n = struct.unpack_from("<4sH", d, i); yield t, d[i+6:i+6+n]; i += 6 + n

SKIN_TONE_INDEX = 24  # NordRace female SkinTone mask

def tint_layers(body):
    """Return [(index, (r,g,b), value)] from TINI/TINC/TINV groups."""
    out = []
    for t, v in subs(body):
        if t == b"TINI": out.append([struct.unpack("<H", v[:2])[0], None, None])
        elif t == b"TINC" and out: out[-1][1] = tuple(v[:3])
        elif t == b"TINV" and out: out[-1][2] = struct.unpack("<i", v[:4])[0]
    return out

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
        continue
    qrgb = tuple(round(x * 255) for x in struct.unpack("<fff", q[:12]))
    skin = [l for l in tint_layers(body) if l[0] == SKIN_TONE_INDEX]
    if not skin:
        print(f"FAIL {edid}: no skin-tone tint layer (TINI {SKIN_TONE_INDEX})"); bad += 1
    elif skin[0][1] != qrgb or not skin[0][2]:
        print(f"FAIL {edid}: skin tint {skin[0][1]} (TINV {skin[0][2]}) != QNAM {qrgb}"); bad += 1
    else:
        print(f"ok   {edid}: skin tint {skin[0][1]} matches: QNAM {tuple(round(x,3) for x in struct.unpack('<fff', q[:12]))}")
print(f"{n} NPC_ checked, {bad} failing"); sys.exit(1 if bad else 0)
