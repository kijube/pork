with open("src/store/api.ts", "r") as f:
    code = f.read()

code = code.replace("?:", ":")

with open("src/store/api.ts", "w") as f:
    f.write(code)