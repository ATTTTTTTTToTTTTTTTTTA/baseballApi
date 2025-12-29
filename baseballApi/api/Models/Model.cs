namespace baseballApi.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
}

public class User
{
    public	string?	n_user_id	{get;set;}=null;
    public	string?	s_username	{get;set;}=null;
    public	string?	s_loginid	{get;set;}=null;
    public	string?	s_mail	{get;set;}=null;
    public	string?	s_password	{get;set;}=null;
    public	string?	s_role	{get;set;}=null;
    public	string?	s_refresh_token	{get;set;}=null;
    public	DateTime?	d_createdate	{get;set;}=null;
    public	DateTime?	d_updatedate	{get;set;}=null;

}