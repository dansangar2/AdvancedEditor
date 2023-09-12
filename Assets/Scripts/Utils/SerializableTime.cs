using System;
using UnityEngine;

namespace Utils
{
    [Serializable]
    public class SerializableTime
    {

        [SerializeField] private int seconds;
        [SerializeField] private int minutes;
        [SerializeField] private int hours;

        public SerializableTime(int hours, int minutes, int seconds)
        {
            this.seconds = seconds;
            this.minutes = minutes;
            this.hours = hours;
        }

        public int Seconds => seconds;

        public int Minutes => minutes;

        public int Hours => hours;

        public void AddSeconds(int s = 1)
        {
            seconds+=s;
            if (seconds >= 60)
            {
                seconds = 0;
                minutes++;
            }

            if (minutes < 60) return;
            minutes = 0;
            if (hours < 999) hours++;
            else hours = 999;
        }
        
        public override string ToString()
        {
            return hours.ToString("D3") + ":" + 
                   minutes.ToString("D2") + ":" + 
                   seconds.ToString("D2");
        }
    }
}