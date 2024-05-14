using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace labyrinth_gamе.Views
{
    public class Diamond
    {
        private Image _image;
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsCollected { get; set; }

        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }
    }
}
