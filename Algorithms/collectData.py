import os

a = "m"
#for a in ("f", "m"):
for b in range(2, 6):
    for i in range(1, 501):
        for t in range (1, 6):
            os.system("dotnet run " + " ".join((a, str(b), str(i), str(t))))
