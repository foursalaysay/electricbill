using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace electricbill
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;
        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<Bill>().Wait();
        }

        //Insert and Update new record  
        public Task<int> SaveBill(Bill bill)
        {
            if (bill.meterNumber != 0)
            {
                return db.UpdateAsync(bill);
            }
            else
            {
                return db.InsertAsync(bill);
            }
        }

        //Delete  
        public Task<int> DeleteBill(Bill bill)
        {
            return db.DeleteAsync(bill);
        }

        //Read All Items  
        public Task<List<Bill>> ReadBill()
        {
            return db.Table<Bill>().ToListAsync();
        }


        //Read Item  
        public Task<Bill> GetItemAsync(int meterno)
        {
            return db.Table<Bill>().Where(i => i.meterNumber == meterno).FirstOrDefaultAsync();
        }
    }

}
