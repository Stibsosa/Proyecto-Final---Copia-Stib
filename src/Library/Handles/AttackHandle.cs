
namespace Battleship
{
    /// <summary>
    /// Un "handler" del patrón Chain of Responsibility que implementa el comando "atacar".
    /// </summary>
    public class AttackHandle : BaseHandler
    {

        private IPrinter Printer;
        private IInputText InputText;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="AttackHandle"/>. Esta clase procesa el mensaje "atacar".
        /// </summary>
        /// <param name="next">El próximo "handler".</param>
        public AttackHandle(BaseHandler next, IPrinter printer, IInputText inputText) : base(next)
        {
            this.Keywords = new string[] {"Atacar", "atacar", "ATACAR"};
            this.Printer = printer;
            this.InputText = inputText;
        }

        /// <summary>
        /// Procesa el mensaje "atacar" y retorna true; retorna false en caso contrario.
        /// </summary>
        /// <param name="message">El mensaje a procesar.</param>
        /// <param name="response">La respuesta al mensaje procesado.</param>
        /// <returns>true si el mensaje fue procesado; false en caso contrario.</returns>
        protected override void InternalHandle(Message message, out string response)
        {
            try
            {
                User user = UserRegister.GetUser(message.id);

                if (user.getStatus() != "in game")
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

                        if (userAttacked.getStatus() != "in game")
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
                    
                    // Ataque
                    Printer.Print(user.GetPlayer().GetBoardsToPrint());

                    Printer.Print(("\nIngrese las coordenadas de ataque con formato LetraNumero (ejemplo: A1)."));
                    string stringCoordinate = InputText.Input();

                    response = Logic.Attack(stringCoordinate, user, userAttacked);

                    // Juego terminado
                    if (userAttacked.GetPlayer().GetShipsAlive() == 0)
                    {
                        // Mensajes para los jugadores
                        response = "¡Felicitaciones!. Has hundido todos los barcos ¡Ganaste!.";
                        User loserUser = game.GetOtherUserById(user.GetID());
                        loserUser.ChangeTextToPrint("El enemigo ha hundido todos tus barcos. Has perdido.");
                        game.AddUserWinner(user);

                        // Cambio de estado y turno
                        user.ChangeStatus(1);
                        userAttacked.ChangeStatus(1); 
                        response += "\n\n------Turno cambiado------\n\n";   
                        Logic.ChangeTurn(message);

                        // Elimina el juego de la lista de juegos
                        GamesRegister.RemoveGame(game);
                    
                        // Guardando juego
                        //GamesRegister.SaveGame(game);
                    }

                    // Cambio de turno
                    if(response == "Agua" || response == "Hundido" || response == "Tocado")
                    {
                        response += "\n\n\n\n------Turno cambiado------\n\n"; 
                        Logic.ChangeTurn(message);
                    }
                }
                
            }
            catch
            {
                response = "Sucedió un error";
            }
        }
    }
}