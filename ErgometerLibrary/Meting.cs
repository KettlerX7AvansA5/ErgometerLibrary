using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgometerLibrary
{
    public class Meting
    {
        public int HeartBeat { get; set; }
        public int RPM { get; set; }
        public double Speed { get; set; }
        public double Distance { get; set; }
        public int Power { get; set; }
        public int Energy { get; set; }
        public int Seconds { get; set; }
        public int ActualPower { get; set; }
        public double TimeStamp { get; }

        public Meting(int heartbeat, int rpm, double speed, double distance, int power, int energy, int seconds, int actualpower, double timestamp)
        {
            HeartBeat = heartbeat;
            RPM = rpm;
            Speed = speed;
            Distance = distance;
            Power = power;
            Energy = energy;
            Seconds = seconds;
            ActualPower = actualpower;
            TimeStamp = timestamp;
        }

        public override string ToString()
        {
            string temp = "";
            temp += "Heartbeat: " + HeartBeat + "\n";
            temp += "RPM: " + RPM + "\n";
            temp += "Speed: " + Speed + "\n";
            temp += "Distance: " + Distance + "\n";
            temp += "Power: " + Power + "\n";
            temp += "Energy: " + Energy + "\n";
            temp += "Seconds: " + Seconds + "\n";
            temp += "ActualPower: " + ActualPower + "\n";
            return temp;
        }

        public string ToCommand()
        {
            string temp = "";
            temp += HeartBeat + "»";
            temp += RPM + "»";
            temp += Speed + "»";
            temp += Distance + "»";
            temp += Power + "»";
            temp += Energy + "»";
            temp += Seconds + "»";
            temp += ActualPower + "»";
            temp += TimeStamp;
            return temp;
        }

        public static Meting Parse(string input)
        {
            return Parse(input, '\t');
        }

        public static Meting Parse(string input, char delimiter)
        {
            string[] status = input.Split(delimiter);
            if (status.Length != 8 && status.Length != 9)
            {
                throw new FormatException("Error in Meting: Arguments do not match");
            }
            int heartbeat = int.Parse(status[0]);
            int rpm = int.Parse(status[1]);
            double speed = double.Parse(status[2]) / 10;
            double distance = double.Parse(status[3]) / 10;
            int power = int.Parse(status[4]);
            int energy = int.Parse(status[5]);
            int actualpower = int.Parse(status[7]);

            double timestamp = 0;
            if (status.Length == 9)
                timestamp = double.Parse(status[8]);
            else
                timestamp = Helper.Now;

            string[] temp = status[6].Split(':');
            int seconds = 0;
            if (temp.Length == 2)
            {
                seconds = (int.Parse(temp[0]) * 60) + (int.Parse(temp[1]));
            }
            else
            {
                seconds = int.Parse(status[6]);
            }

            return new Meting(heartbeat, rpm, speed, distance, power, energy, seconds, actualpower,timestamp);
        }
    }
}
