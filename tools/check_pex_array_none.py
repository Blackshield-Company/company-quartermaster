#!/usr/bin/env python3
"""Fail if a compiled Skyrim script compares an array with None.

`arr == None` compiles to `cast ::tempN None` with ::tempN typed as an array
(Actor[], Int[], ...). The game rejects that cast ("Cannot cast from None to
Actor[]"), the temp loses its array type, and a following `new Actor[8]` that
reuses the temp fails with "Cannot create an array into a non-array variable".
Guard arrays with a flag or `!arr` instead. Both Bethesda's compiler and the
open-source one emit this, so only the bytecode shows it.

Usage: check_pex_array_none.py <file.pex> [...]    (format: UESP, big-endian)
"""
import struct
import sys

FIX = {0: 0, 1: 3, 2: 3, 3: 3, 4: 3, 5: 3, 6: 3, 7: 3, 8: 3, 9: 3, 10: 2, 11: 2, 12: 2, 13: 2,
       14: 2, 15: 3, 16: 3, 17: 3, 18: 3, 19: 3, 20: 1, 21: 2, 22: 2, 23: 3, 24: 2, 25: 3,
       26: 1, 27: 3, 28: 3, 29: 3, 30: 2, 31: 2, 32: 3, 33: 3, 34: 4, 35: 4, 36: 3}
VARARGS = {23, 24, 25}  # callmethod, callparent, callstatic
CAST = 14


class Reader:
    def __init__(self, data):
        self.d, self.o = data, 0

    def take(self, fmt):
        v = struct.unpack_from(">" + fmt, self.d, self.o)[0]
        self.o += struct.calcsize(">" + fmt)
        return v

    def wstr(self):
        n = self.take("H")
        s = self.d[self.o:self.o + n].decode("latin-1")
        self.o += n
        return s


def check(path):
    r = Reader(open(path, "rb").read())
    if r.take("I") != 0xFA57C0DE:
        raise SystemExit(f"{path}: not a Skyrim PEX file")
    r.take("B"), r.take("B"), r.take("H"), r.take("Q")
    r.wstr(), r.wstr(), r.wstr()
    strings = [r.wstr() for _ in range(r.take("H"))]
    if r.take("B"):  # debug info
        r.take("Q")
        for _ in range(r.take("H")):
            r.take("H"), r.take("H"), r.take("H"), r.take("B")
            n = r.take("H")
            r.o += 2 * n
    for _ in range(r.take("H")):  # user flags
        r.take("H"), r.take("B")

    def value():
        t = r.take("B")
        if t in (1, 2):
            return ("id" if t == 1 else "str", strings[r.take("H")])
        if t == 3:
            return ("int", r.take("i"))
        if t == 4:
            return ("float", r.take("f"))
        if t == 5:
            return ("bool", r.take("B"))
        return ("none", None)

    bad = []

    def function(name):
        r.take("H"), r.take("H"), r.take("I"), r.take("B")
        for _ in range(r.take("H")):
            r.take("H"), r.take("H")
        local_types = {}
        for _ in range(r.take("H")):
            n = strings[r.take("H")]
            local_types[n] = strings[r.take("H")]
        for _ in range(r.take("H")):
            op = r.take("B")
            args = [value() for _ in range(FIX[op])]
            if op in VARARGS:
                for _ in range(value()[1]):
                    value()
            if op == CAST and len(args) == 2 and args[1][0] == "none":
                dest = args[0][1]
                if local_types.get(dest, "").endswith("[]"):
                    bad.append(f"{name}: cast {dest} ({local_types[dest]}) None")

    for _ in range(r.take("H")):  # objects
        obj = strings[r.take("H")]
        r.take("I"), r.take("H"), r.take("H"), r.take("I"), r.take("H")
        for _ in range(r.take("H")):  # variables
            r.take("H"), r.take("H"), r.take("I")
            value()
        for _ in range(r.take("H")):  # properties
            pname = strings[r.take("H")]
            r.take("H"), r.take("H"), r.take("I")
            flags = r.take("B")
            if flags & 4:
                r.take("H")
            else:
                if flags & 1:
                    function(f"{obj}.{pname}.get")
                if flags & 2:
                    function(f"{obj}.{pname}.set")
        for _ in range(r.take("H")):  # states
            r.take("H")
            for _ in range(r.take("H")):
                function(f"{obj}.{strings[r.take('H')]}")

    return bad


def main(paths):
    failed = False
    for p in paths:
        for line in check(p):
            print(f"{p}: array compared with None: {line}", file=sys.stderr)
            failed = True
    return 1 if failed else 0


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(__doc__)
        sys.exit(2)
    sys.exit(main(sys.argv[1:]))
