public class TurnManager
{
    public int Turn => turn;
    private int turn = 0;

    public void NextTurn()
    {
        turn++;
        
    }
}