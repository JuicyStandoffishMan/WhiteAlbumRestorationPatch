import os
import win32com.client as win32com


in_dir = os.path.join(os.getcwd(), 'stripped')

for filename in os.listdir(in_dir):
    if filename.endswith('.xlsx'):
        file_path = os.path.join(in_dir, filename)

        book = file_path
        excel = win32com.gencache.EnsureDispatch('Excel.Application')
        wb = excel.Workbooks.Open(book, Local=True)
        wb.RemovePersonalInformation = True
        wb.Close(SaveChanges=1)
        excel.Quit()

        print(f"Processed {filename}")