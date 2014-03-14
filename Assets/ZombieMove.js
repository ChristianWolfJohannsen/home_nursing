var distance;
var target : Transform;    
var lookAtDistance = 15.0;
var attackRange = 10.0;
var moveSpeed = 5.0;
var damping = 6.0;
private var isItAttacking = false;
 
function Update () 
{
    distance = Vector3.Distance(target.position, transform.position);
 
    if(distance < lookAtDistance)
    {
        isItAttacking = false;
        renderer.material.color = Color.yellow;
        lookAt ();
    }   
    if(distance > lookAtDistance)
    {
        renderer.material.color = Color.green; 
    }
    if(distance < attackRange)
    {
        attack ();
    }
    if(isItAttacking)
    {
        renderer.material.color = Color.red;
    }
}
 
 
function lookAt ()
{
    var rotation = Quaternion.LookRotation(target.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
}
 
function attack ()
{
    isItAttacking = true;
    renderer.material.color = Color.red;
 
    transform.Translate(Vector3.forward * moveSpeed *Time.deltaTime);
}