using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class MovimientosFichas
    {
    
        static string[] blancas = { "♖", "♘", "♗", "♕", "♙", "♔"};
        static string[] negras = { "♜", "♞", "♝", "♛", "♚", "♟" };
        
        public static bool ValidarIndiceFueraDeRango(int[] posicion)
        {
            bool indiceFueraDeRango = !(0 <= posicion[0] && posicion[0] < 8 && 0 <= posicion[1] && posicion[1] < 8);
            return indiceFueraDeRango;
        }
            
        public static List<int[]> ListaPosiblesAtaquesPeon(int px, int py, bool esBlanco, string[,] tablero)
        {
            List<int[]> movimientos = new List<int[]>();
            string[] enemigos = esBlanco ? negras : blancas;
            List<int[]> posiblesAtaques = new List<int[]>([[px - 1, py - 1], [px - 1, py + 1], [px + 1, py - 1], [px + 1, py + 1]]);
            foreach (int[] posicion in posiblesAtaques)
            {
                if (ValidarIndiceFueraDeRango(posicion)) continue;
                string fichaActual = tablero[posicion[0], posicion[1]];
                if (!enemigos.Contains(fichaActual)) continue;
                movimientos.Add([posicion[0], posicion[1]]);
            }
            return movimientos;
        }

        public static List<int[]> ListaMovimientosPeon(int px, int py, bool esBlanco, string[,] tablero)
        {
            List<int[]> movimientos = ListaPosiblesAtaquesPeon(px, py, esBlanco, tablero);
            bool esPrimerMovimiento = (px == 1 && esBlanco) || (px == 6 && !esBlanco);
            int multiplicador = esBlanco ? 1 : -1;
            for (int i = 1; i <= (esPrimerMovimiento ? 2 : 1); i++)
            {

                if (ValidarIndiceFueraDeRango([px + (i * multiplicador), py])) break;
                if (!(tablero[px + (i * multiplicador), py] == "")) break;
                movimientos.Add([px + (i * multiplicador), py]);
            }
            return movimientos;
        }

        private static List<int[]> MovimientosDirigido(int px, int py, bool esBlanco, string[,] tablero, List<List<int>> sumadores)
        {
            List<int[]> movimientos = new List<int[]>();
            foreach (List<int> sumador in sumadores)
            {
                int[] posicionActual = new int[] { px, py };
                bool fichaEncontrada = false;
                while (!fichaEncontrada)
                {
                    posicionActual = new int[] { posicionActual[0] + sumador[0], posicionActual[1] + sumador[1] };
                    if (ValidarIndiceFueraDeRango(posicionActual)) break;
                    string fichaActual = tablero[posicionActual[0], posicionActual[1]];
                    string[] fichasEnemigas = esBlanco ? negras : blancas;
                    if (!string.IsNullOrEmpty(fichaActual) && !fichasEnemigas.Contains(fichaActual))
                    {
                        fichaEncontrada = true;
                        break;
                    }
                    movimientos.Add(new int[] { posicionActual[0], posicionActual[1] });
                    if (fichasEnemigas.Contains(fichaActual)) fichaEncontrada = true;
                }
            }
            return movimientos;
        }

        public static List<int[]> ListaMovimientosAlfil(int px, int py, bool esBlanco, string[,] tablero)
        {
            List<List<int>> sumadorAlfil = new List<List<int>>([[1, 1], [1, -1], [-1, 1], [-1, -1]]);
            return MovimientosDirigido(px, py, esBlanco, tablero, sumadorAlfil);
        }
        public static List<int[]> ListaMovimientosTorre(int px, int py, bool esBlanco, string[,] tablero)
        {
            List<List<int>> sumadorTorre = new List<List<int>>([[1, 0], [0, 1], [-1, 0], [0, -1]]);
            return MovimientosDirigido(px, py, esBlanco, tablero, sumadorTorre);
        }
        public static List<int[]> ListaMovimientosReina(int px, int py, bool esBlanco, string[,] tablero)
        {
            List<int[]> ListaMovimientosDiagonales = ListaMovimientosAlfil(px, py, esBlanco, tablero);
            List<int[]> ListaMovimientosRectos = ListaMovimientosTorre(px, py, esBlanco, tablero);
            ListaMovimientosDiagonales.AddRange(ListaMovimientosRectos);
            return ListaMovimientosDiagonales;
        }

        public static List<int[]> ListaMovimientosRey(int px, int py, bool esBlanco, string[,] tablero)
        {
            
            List<int[]> movimientosRey = new List<int[]>([[0, -1], [-1, 0], [1, 0], [0, 1],
                                                       [- 1, - 1], [- 1, 1], [1, - 1], [1,  1]]);
            List<int[]> movimientos = new List<int[]>();
            foreach (int[] movimiento in movimientosRey)
            {
                int[] posicionActual = [px, py];
                posicionActual[0] += movimiento[0];
                posicionActual[1] += movimiento[1];
                if (ValidarIndiceFueraDeRango(posicionActual)) continue;
                string fichaActual = tablero[posicionActual[0], posicionActual[1]];
                string[] fichasEnemigas = esBlanco ? negras : blancas;
                if (fichaActual != "" && !(fichasEnemigas.Contains(fichaActual))) continue;
                movimientos.Add(new int[] { posicionActual[0], posicionActual[1] });
            }
            return movimientos;
        }

        public static List<int[]> ListaMovimientosCaballo(int px, int py, bool esBlanco, string[,] tablero)
        {
            List<int[]> movimientos = new List<int[]>();
            List<List<int>> multiplicadores = new List<List<int>>([[1, 1], [1, -1], [-1, 1], [-1, -1]]);
            List<List<int>> posiblesMovimientos = new List<List<int>>([[2, 1], [1, 2]]);
            foreach (List<int> multiplicador in multiplicadores)
            {
                foreach (List<int> movimiento in posiblesMovimientos)
                {
                    int[] posicionActual = [px, py];
                    posicionActual[0] += movimiento[0] * multiplicador[0];
                    posicionActual[1] += movimiento[1] * multiplicador[1];
                    if (ValidarIndiceFueraDeRango(posicionActual)) continue;
                    string fichaActual = tablero[posicionActual[0], posicionActual[1]];
                    string[] fichasEnemigas = esBlanco ? negras : blancas;
                    if (fichaActual != "" && !(fichasEnemigas.Contains(fichaActual))) continue;
                    movimientos.Add(new int[] { posicionActual[0], posicionActual[1] });
                }
            }
            return movimientos;
        }
    }
}
