using System;
using System.Collections.Generic;
using System.Text;

namespace CCD
{
    public class CCDViewModel
    {
        public Bone Selectedbone { get; set; }
        public RelayCommand<object> AddBoneCommand { get; set; }

    }
}
