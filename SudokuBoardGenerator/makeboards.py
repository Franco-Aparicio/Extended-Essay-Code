import os

for b in range(2, 6):
    for i in range(1, 501):
        os.system("dotnet run " + str(b))
