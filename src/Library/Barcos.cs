using System;
using System.Collections;
namespace BatallaNaval
{
    public class Barco
    {
        public ArrayList poss = new ArrayList();


        public int Size{get;private set;}
        public int Id{get;private set;}

        public Barco(int id,int size, int pos1x, int pos1y, int pos2x, int pos2y){
            
            if ((Math.Max(pos1x,pos2x) - Math.Min(pos1x,pos2x)) < 1 && (Math.Max(pos1y,pos2y) - Math.Min(pos1y,pos2y)) < 1){
                Console.WriteLine("No es posible la creacion del barco puesto que las coordenadas no concuerdan con el tamaño del barco");
            }
            else if ((Math.Max(pos1x,pos2x) - Math.Min(pos1x,pos2x)) == (size - 1) || (Math.Max(pos1y,pos2y) - Math.Min(pos1y,pos2y)) == (size-1) )
            {
                this.Id = id;
                this.Size = size;
                this.poss.Add($"{pos1x},{pos1y}");
                this.poss.Add($"{pos2x},{pos2y}");
            }
            else{
                Console.WriteLine("No es posible la creacion del barco puesto que las coordenadas no concuerdan con el tamaño del barco");

            }
        }

    }
}