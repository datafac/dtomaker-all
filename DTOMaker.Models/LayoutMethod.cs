namespace DTOMaker.Models;

/// <summary>
/// The entity's member memory layout method. This need only be defined when using
/// DTO source generators that require or support memory layout, such as MemBlocks.
/// </summary>
public enum LayoutMethod : int
{
    /// <summary>
    /// No member memory layout method is defined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Member memory layout is explicitly defined by the model designer. This is
    /// only used by DTO source generators that support memory layout, such as 
    /// MemBlocks. When this method is used: a) the entity block length must be
    /// explicitly set; b) each member must have a [Offset] attribute that
    /// defines its unique and immutable location within the entity memory block; 
    /// c) each member data type must be a non-nullable value type or fixed length
    /// array of these, or string.
    /// </summary>
    Explicit = 1,

    /// <summary>
    /// Member memory layout is automatic and is assigned in sequence order.
    /// The location assigned is always the next higher available block with
    /// the correct size and alignment.
    /// 
    /// It is important that the member sequence and data type do not ever 
    /// change, to ensure consistent backward compatibility. With this
    /// method, the entity memory block length can be limited, or left undefined
    /// to allow automatic growth.
    /// </summary>
    Linear = 2,

    /// <summary>
    /// Member memory layout is automatic and is assigned in sequence order.
    /// The location assigned is always the first available, of the correct
    /// size and alignment, closest to the previously assigned member.
    /// 
    /// It is important that the member sequence and data type do not ever 
    /// change, to ensure consistent backward compatibility. With this
    /// method, the entity memory block length can be limited, or left undefined
    /// to allow automatic growth.
    /// </summary>
    Compact = 3, // todo implement and make default
}

