namespace X01{
public sealed class RelayUri : Uri{

}
public static class RelayUriExt {
public static RelayUri Relay(this Uri uri){
return (RelayUri(uri));
}
}
}
