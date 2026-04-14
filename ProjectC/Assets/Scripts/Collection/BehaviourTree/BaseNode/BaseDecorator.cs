/// <summary>
/// [Decorator Node] 
/// Decorator Pattern을 사용하여 하나의 자식 노드만을 Wrapping 하고 관리하는 추상 클래스다.
/// 자식 노드의 실행 조건이나 반환되는 Status를 동적으로 변경하는 기능을 추가한다.
/// 해당 클래스를 상속하는 클래스는 자식을 실행하고 결과를 수정하는 Execute() 로직을 구현해야 한다.
/// </summary>
public abstract class BaseDecorator : IBehaviour
{
    protected IBehaviour child;

    // 자식 노드
    public BaseDecorator(IBehaviour child)
    {
        this.child = child;
    }

    public abstract Status Execute(Blackboard blackBoard);
}
