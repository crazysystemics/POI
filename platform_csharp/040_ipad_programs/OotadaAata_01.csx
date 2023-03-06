using static System.Console;

meals_game.parse(ref first_line_list, ref second_line_list)
{

}

//procedure
nested_string<> Convert_OrderedList_To_Sequence(string<> input_list)
{
       if every adjacent pair in input_list is ordered
		return “is_sequence“ + input_list;
       else
		return input_list;
}

//procedure
nested_string<> Convert_List_To_Sequence_By_Analogy(string<> referred_sequence,
 											  ref string<> referring_list)
{
	if first term of referred sequence is “is_sequence”
	and
	if first_list.analogy_group_id == second_list.analogy_group_id
		return “is_sequence” + referring_list;
	else
		return referring_list; 
}	

nested_string next(nested_string<> p_sequence, nested_string token)
{
	if first_term of p_sequence is “is_sequence”
        {
		int pos = index of token in p_sequence;
                if pos is last token in p_sequence
 			return “end_point”;
		else
			return p_sequence[pos + 1];
	}
}

nested_string  prev(nested_string<> p_sequence, nested_string token) // function
{
	if first_term of p_sequence is “is_sequence” // predicate (boolean function)
        {
		int pos = index of token in p_sequence;
                if pos is first token in p_sequence
 			return “starting_point”;
		else
			return p_sequence[pos - 1];
	}
}


class NestedStringSequenceIterator
{
	nested_string<> nested_string_sequence;
	nested_string begin();
        nested_string end();

	nested_string operator *();
        nested_string operator++();
        nested_string  operator—();
}






WriteLine ("Hello World!");
