public static class PredicateLibrary
{
    public static bool isClear(object[] args)
    {
        Block block = args[0] as Block;
        return block != null && block.isClear();
    }

    public static bool isOn(object[] args)
    {
        return true;
    }
}
