using System.Collections.Generic;

/// <summary>
/// [Composite Node] or [Control Node]
/// 여러 자식 노드들을 관리하고 실행 흐름을 제어하는 추상 클래스다.
/// Composite Pattern에서 Composite or Control 역할을 수행한다.
/// 해당 클래스를 상속하는 클래스는 AddChild로 추가된 자식들을 순회하며 실행 흐름을 결정하는 Execute() 로직을 구현해야 한다.
/// </summary>
public abstract class BaseComposite : IBehaviour
{
    protected List<IBehaviour> children = new List<IBehaviour>();

    public BaseComposite(List<IBehaviour> _children)
    {
        this.children = _children ?? new List<IBehaviour>();
    }

    public void AddChlid(IBehaviour behaviour)
    {
        children.Add(behaviour);
    }

    public abstract Status Execute(Blackboard blackBoard);
}
