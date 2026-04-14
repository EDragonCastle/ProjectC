using UnityEngine;

[CreateAssetMenu(fileName = "NewCardTransformSetting", menuName = "Setting/Card Transform Setting")]
public class CardTransformSetting : ScriptableObject
{
    public CardTransform mask;
    public CardTransform cardMainImage;
    public CardTransform legandPortrait;

    public CardTransform cardExplanation;

    public CardTransform gem;
    public CardTransform cardName;
    public CardTransform cardNameText;
    public CardTransform cardType;
    public CardTransform cardTypeText;
    public CardTransform attack;
    public CardTransform attackText;
    public CardTransform health;
    public CardTransform healthText;

    public bool isVoidValue(CardTransform value)
    {
        if (value.position == Vector2.zero && value.ratio == Vector2.zero && value.scale == Vector3.zero)
            return true;
        else
            return false;
    }
}
