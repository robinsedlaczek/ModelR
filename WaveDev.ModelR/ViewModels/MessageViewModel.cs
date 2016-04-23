using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveDev.ModelR.ViewModels
{
    public class MessageViewModel : ViewModelBase
    {
        private static int s_lastId = 0;

        public MessageViewModel(string message)
        {
            Id = ++s_lastId;

            Message = message;
        }

        public int Id { get; }

        public string Message { get; }

    }
}
