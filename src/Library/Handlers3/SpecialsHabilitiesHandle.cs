using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    /// <summary>
    /// Un "handler" del patrón Chain of Responsibility que implementa los comandos "ataque aereo","satelite".
    /// </summary>
    public class SpecialHabilitiesHandler : BaseHandler
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SpecialHabilitiesHandler"/>. Esta clase procesa el mensaje "aereo","satelite".
        /// </summary>
        /// <param name="next">El próximo "handler".</param>
        public SpecialHabilitiesHandler(BaseHandler next) : base(next)
        {
            this.Keywords = new string[] {"aereo", "satelite"};
        }

        /// <summary>
        /// Procesa los mensajes "aereo", "vidente", "satelite" y retorna true; retorna false en caso contrario.
        /// </summary>
        /// <param name="message">El mensaje a procesar.</param>
        /// <param name="response">La respuesta al mensaje procesado.</param>
        /// <returns>true si el mensaje fue procesado; false en caso contrario.</returns>
        protected override void InternalHandle(Message message, out string response)
        {
            try
            {
                User user = UserRegister.GetUser(message.id);

                if (user.getStatus() != $"in {user.GetGameMode()} game")
                {
                    // Estado de user incorrecto
                    response = $"Comando incorrecto. Estado del usuario = {user.getStatus()}";
                    return;
                }
                else
                {
                    Game game = null;
                    User userAttacked = null;
                    try
                    {
                        // Accediendo al otro usuario(player)
                        int gameId = user.GetPlayer().GetGameId();
                        game = GamesRegister.GetGameInPlay(gameId);

                        userAttacked = game.GetOtherUserById(user.GetID());

                        if (userAttacked.getStatus() != $"in {user.GetGameMode()} game")
                        {
                            response = "El contricante no ha posicionado los barcos.";
                            return;
                        }
                    }
                    catch
                    {
                        response = "Error - No se encontró al otro usuario.";
                        return;
                    }
                    
                    try
                    {
                        string[] direction = message.Text.Split(' ');

                        if (message.Text == $"aereo {direction[1]}" || message.Text == $"aereo {direction[1]}" || message.Text == $"AEREO {direction[1]}")
                        {
                            if (!user.GetPlayer().GetSpecialsHabilities().Contains("air attack"))
                            {
                                response = "Ya has utilizado la habilidad ataque aereo";
                                return;
                            }

                            string theRow = direction[1];

                            if (!Logic.GetRow().Contains(theRow.ToUpper()))
                            {
                                response = "Fila ingresada incorrecta";
                                return;
                            }

                            Logic.AirAttack(theRow, user, userAttacked);

                            user.GetPlayer().UseHability("air attack");

                            response = "Fila atacada con exito";
                        }
                        else if (message.Text == $"satelite {direction[1]}" || message.Text == $"Satelite {direction[1]}" || message.Text == $"SATELITE {direction[1]}")
                        {
                            if (!user.GetPlayer().GetSpecialsHabilities().Contains("satellite photo"))
                            {
                                response = "Ya has utilizado la habilidad satelite";
                                return;
                            }

                            string theColumn = direction[1];

                            List<string> validColumns = new List<string>{"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};

                            if (!validColumns.Contains(theColumn))
                            {
                                response = "Columna ingresada incorrecta";
                                return;
                            }

                            int columnInt = int.Parse(theColumn);

                            response = "Foto satelital recibida:\n";

                            response += Logic.Satelitte(columnInt, userAttacked.GetPlayer().GetShipsBoard().GetBoard());

                            user.GetPlayer().UseHability("satellite photo");
                        }
                        else
                        {
                            throw new Exception();
                        }

                        response += "\n\n\n\n------Turno cambiado------\n\n"; 
                        Logic.ChangeTurn(message);
                    }
                    catch
                    {
                        response = "Sucedió un error";
                    }
                } 
            }
            catch
            {
                response = "Sucedió un error";
            }
        }

        protected override bool CanHandle(Message message)
        {
            try
            {
                string[] words = message.Text.Split(' ');


                if (this.Keywords.Contains(words[0]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            
        }
        
    }
}