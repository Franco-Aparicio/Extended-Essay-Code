from openpyxl import Workbook
wb = Workbook()
ws1 = wb.active
ws1.title = "Size Averages"
ws2 = wb.create_sheet(title="Board Averages")

def main():
    path = "./raw/"
    for a in ("f", "m"):
        for s in range(2, 6):
            sTotal = [0, 0, 0, 0]
            for b in range(1, 501):
                average = getBoardAverage(path, a, s, b)
                for d in range(4):
                    sTotal[d] += average[d]
            for d in range(4):
                sTotal[d] = float(sTotal[d]/500)
                if a == "m":
                    ws1.cell(row=s-1, column=6+d, value=sTotal[d])
            if a == "f":
                ws1.append([s] + sTotal)
    wb.save("processed.xlsx")

def getBoardAverage(path, a, s, b):
    totals = [0, 0, 0, 0]
    for t in range(1, 6):
        with open(f"{path}{'.'.join((a, str(s), str(b)))}.{t}.txt", "r") as f:
            lines = f.readlines()
            for l in range(4):
                totals[l] += int(lines[l])
    for d in range(4):
        totals[d] = totals[d]/5
        if a == "m":
            ws2.cell(row=((s-2)*500)+b, column=6+d, value=totals[d])
    if a == "f":
        ws2.append([f"{s}.{b}"] + totals)
    return totals

main()
