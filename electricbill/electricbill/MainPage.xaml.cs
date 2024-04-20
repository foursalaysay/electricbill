using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace electricbill
{
    public partial class MainPage : ContentPage
    {

        public double consumption;
        public double eCharge;
        public double eKiloHour;
        public double dCharge;
        public double sCharge;
        public double principal;
        public double vat;
        public double amountpayable;

        public long meterno;
        public double presentRead;
        public double previousRead;
        public string registerType;

        public Boolean meternoTrue = false;
        public Boolean presentRTrue = false;
        public Boolean previousRTrue = false;
        public Boolean rTypeTrue = false;

        public MainPage()
        {
            InitializeComponent();
            this.Appearing += MainPage_Appearing;
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
            string registerType = ""; // Assuming you have a variable named registerType

            if (registerType.ToUpperInvariant() != "H" && registerType.ToUpperInvariant() != "B")
            {
                DisplayAlert("Invalid Input", "Please enter valid input.", "OK");
            }
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Get All Persons  
            var personList = await App.SQLiteDb.ReadBill();
            if (personList != null)
            {
                Bill.ItemsSource = personList;
            }
        }




        // FOR ADDING VALUE
        async void Button_Clicked(object sender, EventArgs e)
        {
            // CHECK IF THERE IS INPUT 
            if (string.IsNullOrWhiteSpace(meterNo.Text))
            {
                await DisplayAlert("Invalid", "You must Enter Meter Number", "Ok");
            }
            else
            {
                meternoTrue = true;
            }
            if (string.IsNullOrWhiteSpace(presentR.Text))
            {
                await DisplayAlert("Invalid", "You must Enter Meter Number", "Ok");
            }
            else
            {
                presentRTrue = true;
            }
            if (string.IsNullOrWhiteSpace(previousR.Text))
            {
                await DisplayAlert("Invalid", "You must Enter Meter Number", "Ok");
            }
            else
            {
                previousRTrue = true;
            }
            if (string.IsNullOrWhiteSpace(rType.Text))
            {
                await DisplayAlert("Invalid", "You must Enter Meter Number", "Ok");
            }
            else
            {
                rTypeTrue = true;
            }

            // CALCULATIONS 
            //---------------------------------------------------------------
            meterno = long.Parse(meterNo.Text);
            presentRead = double.Parse(presentR.Text);
            previousRead = double.Parse(previousR.Text);
            registerType = rType.Text;


            consumption = consumptionReading(presentRead, presentRead);
            eKiloHour = eKilowattHour(consumption);
            eCharge = electricityCharge(consumption, eKiloHour);
            dCharge = demandCharge(registerType);
            sCharge = serviceCharge(registerType);
            principal = principalAmount(eCharge, dCharge, sCharge);
            vat = vatAmount(principal);
            amountpayable = amountPayable(principal, vat);

            //---------------------------------------------------------------

            if (meternoTrue && presentRTrue && previousRTrue && rTypeTrue)
            {
                addRecord();
            }
            else
            {
                await DisplayAlert("Invalid", "Your data is not recorded.", "Ok");
            }


        }

        private double consumptionReading(double firstRead, double lastRead)
        {
             double billRead = lastRead - firstRead;
              return billRead;
        }

        
        private double electricityCharge(double CR, double EKH)
        {
            return CR * EKH;
        }
        private double eKilowattHour(double CR)
        {
            double EC;

            if(CR < 72)
            {
                EC = 6.50;
            }else if(CR >= 72 && CR <= 150)
            {
                EC = 9.50;
            }
            else if (CR > 150 && CR <= 300)
            {
                EC = 10.50;
            }
            else if (CR > 300 && CR <= 400)
            {
                EC = 12.50;
            }
            else if (CR > 400 && CR <= 500)
            {
                EC = 14;
            }
            else
            {
                EC = 16.50;
            }
            return EC;
        }
        
        
        private double demandCharge(string typeR)
        {
            double dValue;
            if (typeR.ToUpperInvariant() == "H") 
            {
                dValue = 200;
            }
            else
            {
                dValue = 400; 
            }
            return dValue; 
        }
        private double serviceCharge(string typeR)
        {
            double sValue;
            if (typeR.ToUpperInvariant() == "H")
            {
                sValue = 50;
            }
            else
            {
                sValue = 100;
            }
            return sValue;
        }

        private double principalAmount(double EC, double DC, double SC)
        {
            return EC * DC * SC;
        }

        private double vatAmount(double PA)
        {
            return PA * 0.05;
        }

        private double amountPayable(double PA, double Vat)
        {
            return PA + Vat;
        }





        async void addRecord()
        {
            await App.SQLiteDb.SaveBill(new electricbill.Bill
            {
                meterNumber = meterno,
                pAmount = principal,
                aPayable = amountpayable
            }); ;
            

        }
    }
}
