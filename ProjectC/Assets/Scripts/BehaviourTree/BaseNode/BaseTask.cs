/// <summary>
/// [Execution Node] or [Task Node] or [Leaf Node]
/// 게임 환경과 상호작용하는 실제 행동이나 조건을 검사하는 로직을 구현하는 추상 클래스다.
/// 해당 클래스를 상속하는 클래스는 실행 결과를 Status로 반환하는 Execute() 로직을 구현해야 한다.
/// </summary>
public abstract class BaseTask : IBehaviour
{
    public abstract Status Execute(Blackboard blackBoard);
}
