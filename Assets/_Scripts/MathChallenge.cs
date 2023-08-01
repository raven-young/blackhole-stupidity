using UnityEngine;

public class MathChallenge
{

    public MathChallenge()
    {

    }

    private string equation; // The equation as a string
    private int resultindex; //The index of the result we use. 

    private int levelMultiplier = 20; //Defines the range of a level

    private int[] term = new int[3]; //three terms of an equation
    private int[] solution = new int[3];//Three possible solutions
    private int[] finalsolution = new int[3];//The possible solutions reorderer
    private int neworder; //Which order is selected in the list below
    private int[,] order = new int[,] { { 0, 1, 2 }, { 0, 2, 1 }, { 1, 0, 2 }, { 1, 2, 0 }, { 2, 0, 1 }, { 2, 1, 0 } };//All possible order for three elements
    private int newposition; //The new position of our correct result inside the new order.
    private (string, string, string, string, int) returnvalue;

    //challenge
    //Delivers two sets of data
    //Challenge: two terms and one operator
    //Solutions: three possible solutions
    //level : the level of the challenge, where the easiest level is 0
    public (string, string, string, string, int) SimpleArithemticChallenge(int level)
    {
        //Build a complete challenge
        term[2] = Random.Range(level * levelMultiplier, (level + 1) * levelMultiplier);
        term[0] = Random.Range(0, term[2] / 2) + term[2] / 3;
        term[1] = term[2] - term[0];
        //Select on of those elements (our result) and build an equation with the other two.
        resultindex = (int)Random.Range(1, 3); //Select between a term and the result > As many addition as subtractions
        switch (resultindex)
        {
            //case 0: equation = $"{term[2]} - {term[1]}"; break; //Conseqeuntially not necessary
            case 1: equation = $"{term[2]} - {term[0]}"; break;
            case 2:
                switch ((int)Random.Range(0, 2))
                {
                    case 0: equation = $"{term[0]} + {term[1]}"; break;
                    case 1: equation = $"{term[1]} + {term[0]}"; break;
                }
                break;
        }
        //Find two more possible results different from the one we have.
        //The fake results should be misleading.
        //A bigger and a smaller one? Two bigger ones? Two smaller ones?
        //+/- 10
        switch (resultindex)
        {
            case 1:
                solution[0] = term[1] - (int)Random.Range(1, 4);
                solution[1] = term[1] + (int)Random.Range(1, 4);
                break;
            case 2:
                solution[0] = term[2] - (int)Random.Range(1, 4);
                solution[1] = term[2] + (int)Random.Range(1, 4);
                break;
        }
        solution[2] = term[resultindex]; //Complete the array 
                                         //Reorder the solutions randomly
        neworder = Random.Range(0, 6);
        for (int i = 0; i < 3; i++)
        {
            finalsolution[i] = solution[order[neworder, i]];
            if (order[neworder, i] == 2) newposition = i;
        }

        returnvalue = (equation, finalsolution[0].ToString(), finalsolution[1].ToString(), finalsolution[2].ToString(), newposition);
        //return $"{level}: {equation}: {term[0]},{term[1]},{term[2]}";
        return returnvalue;
    }

    // Challenge: Linear equation of the form a*x +/- b = c
    public (string, string, string, string, int) SimpleAlgebraChallenge(int level)
    {
        int a = Random.Range(1+level, 9 + level);
        int x = Random.Range(1 + level, 9 + level);
        int b = Random.Range(1 + level, 9 + level);
        int constantSign = Random.Range(0f, 1f) < 0.5f ? 1 : -1;
        int c = constantSign > 0 ? a * x + b : a * x - b;
        char sign = constantSign > 0 ? '+' : '-';
        string equation = $"{a}x {sign} {b} = {c}";

        // Find two more fake solutions
        int maxIndex = Random.Range(0, 3); // which of the three solutions is the largest
        int xFake1 = x;
        int xFake2 = x;
        int offset = Random.Range(1, 3);
        switch (maxIndex)
        {
            case 0:
                xFake1 += offset;
                xFake2 += offset + Random.Range(1, 3);
                break;
            case 1:
                xFake1 -= offset;
                xFake2 += Random.Range(1, 3);
                break;
            case 2:
                xFake1 -= offset;
                xFake2 -= offset + Random.Range(1, 3);
                break;
        }

        Debug.Assert(x != xFake2 && x != xFake1);
        // Permute the three solutions
        int[] solutions = new int[3];
        int correctSolutionIndex = Random.Range(0, 3);
        solutions[correctSolutionIndex] = x;
        switch (correctSolutionIndex)
        {
            case 0:
                solutions[1] = xFake1;
                solutions[2] = xFake2;
                break;
            case 1:
                solutions[0] = xFake1;
                solutions[2] = xFake2;
                break;
            case 2:
                solutions[0] = xFake1;
                solutions[1] = xFake2;
                break;
        }
        return (equation, solutions[0].ToString(), solutions[1].ToString(), solutions[2].ToString(), correctSolutionIndex);
    }

}