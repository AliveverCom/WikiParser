using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP_Segmentation
{
    public class CTimeCounter
    {
        protected DateTime StartTime ;

        protected DateTime EndTime ;

        public bool IsEnded = false;

        public string Name = null;

        public CTimeCounter(bool _startNow)
        {
            if (_startNow)
                this.StartTime = DateTime.Now;
        }

        public DateTime SetStart()
        {
            this.StartTime = DateTime.Now;
            IsEnded = false;
            EndTime = DateTime.MinValue;

            return this.StartTime;
        }

        public DateTime SetStart(string _newName)
        {
            this.Name = _newName;

            return this.SetStart();
        }

        public DateTime SetEnd()
        {
            this.EndTime = DateTime.Now;
            this.IsEnded = true;
            return this.EndTime;

        }

        public double GetTotalSeconds()
        {
            if (this.IsEnded)
                return (this.EndTime - this.StartTime).TotalSeconds;
            else
                throw new Exception("CTimeCounter is not ended. There is no total seconds now");
        }

        public double GetSpentSeconds()
        {
            if (this.IsEnded)
                return (this.EndTime - this.StartTime).TotalSeconds;
            else
                return (DateTime.Now - this.StartTime).TotalSeconds;
        }

        public string GetSpentSeconds_Str()
        {
            //string formatStr = "HH:mm:ss";
            TimeSpan ts;
            if (this.IsEnded)
                ts=(this.EndTime - this.StartTime);
            else
                ts=(DateTime.Now - this.StartTime);

            return $"{ts.Hours}:{ts.Minutes}:{ts.Seconds}.{ts.Milliseconds}";
        }

        public string GetSpentSeconds_StrWithName()
        {
            //string formatStr = "HH:mm:ss";
            TimeSpan ts;
            if (this.IsEnded)
                ts=(this.EndTime - this.StartTime);
            else
                ts=(DateTime.Now - this.StartTime);

            return $"'{this.Name}' spent {ts.Hours}:{ts.Minutes}:{ts.Seconds}.{ts.Milliseconds}";
        }

    }//CTimeCounter
}
