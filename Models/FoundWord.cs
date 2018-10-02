using System;
using System.Collections.Generic;
using System.Globalization;

namespace test.Models
{
    public class FoundWord{
       public List<FoundLetter> Hit {get;set;}

    }
    public class FoundLetter{
        public FoundLetter(int row, int column){
            this.Row=row;
            this.Column=column;
        }
        
        public int Row  {get;set;}
        public int Column {get;set;}

    }
}