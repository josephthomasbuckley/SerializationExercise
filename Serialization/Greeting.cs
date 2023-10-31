namespace Serialization;

/* Greeting object, with three properties -- from
   and to Person, and a message.
*/
public class Greeting
{
    
    public Person From { get; set; } = new Person();
    
    public Person To { get; set; } = new Person();
    
    public string Message { get; set; } = string.Empty;


}