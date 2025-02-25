namespace X01
{
    public static class ObjectExt{
          public static bool IsRecycled(this object obj) {
            try {
                obj?.GetType();
                return (false);
            } catch(ObjectDisposedException) {
                return (true);
            }
        }
    }
}
