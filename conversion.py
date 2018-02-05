import csv
import sqlite3


conn = sqlite3.connect('example.db')

cur = conn.cursor()

cur.execute("CREATE TABLE t (ID,Make,Model,Year,Trim,Description,Class,URL,Body type,Length (in),Width (in),Height (in),Wheelbase (in),Curb weight (lbs),Gross weight (lbs),Cylinders,Horsepower (HP),Valves,Valve timing,Drive type,Transmission,Engine type,Fuel type,Fuel tank capacity (gal),EPA mileage (mpg),Front head room (in),Front hip room (in),Front leg room (in),Front shoulder room (in),Rear head room (in),Rear hip room (in),Rear leg room (in),Rear shoulder room (in),Cargo capacity (cuft));")

"""
with open('Car_Dataset.csv','rb') as fin:
    dr = csv.DictReader(fin)
    print(dr)
    for i in dr:
        print(i)
    to_db = [(i['col1'], i['col2']) for i in dr]
"""

rows = []
with open('Car_Dataset.csv', newline='') as csvfile:
    spamreader = csv.reader(csvfile, delimiter=' ', quotechar='|')
    for row in spamreader:
        print(', '.join(row))

cur.executemany("INSERT INTO t (col1, col2) VALUES (?, ?);", to_db)
con.commit()
con.close()
