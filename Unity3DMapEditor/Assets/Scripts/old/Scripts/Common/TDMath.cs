using System;

public struct fVector2
{

    //inline fVector2& operator = ( const fVector2& rkVector )
    //{
    //    x = rkVector.x;
    //    y = rkVector.y;
    //    return *this;
    //}

	public static bool operator == ( fVector2 lVector, fVector2 rVector ) 
	{
		return ( lVector.x == rVector.x && lVector.y == rVector.y );
	}

	public static bool operator != ( fVector2 lkVector, fVector2 rkVector )
	{
		return ( lkVector.x != rkVector.x || lkVector.y != rkVector.y );
	}

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

	public static fVector2 operator + (fVector2 lkVector, fVector2 rkVector ) 
	{
		fVector2 kSum;

		kSum.x = lkVector.x + rkVector.x;
		kSum.y = rkVector.y + rkVector.y;

		return kSum;
	}

	public static fVector2 operator - ( fVector2 lkVector, fVector2 rkVector )
	{
		fVector2 kDiff;

		kDiff.x = lkVector.x - rkVector.x;
		kDiff.y = lkVector.y - rkVector.y;

		return kDiff;
	}

	public static fVector2 operator * (fVector2 lkVector, float fScalar )
	{
		fVector2 kProd;

		kProd.x = fScalar*lkVector.x;
		kProd.y = fScalar*lkVector.y;

		return kProd;
	}

	public static fVector2 operator * ( float fScalar, fVector2 rkVector )
	{
		fVector2 kProd;

		kProd.x = fScalar * rkVector.x;
		kProd.y = fScalar * rkVector.y;

		return kProd;
	}


	public float length(){
       return (float)Math.Sqrt(x * x + y * y);
    }

	public float normalise(){

        float fLength = (float)Math.Sqrt(x * x + y * y);

        // Will also work for zero-sized vectors, but will change nothing
        if (fLength > 1e-08)
        {
            float fInvLength = 1.0f / fLength;
            x *= fInvLength;
            y *= fInvLength;
        }

        return fLength;
    }


	public fVector2(float _x, float _y){
        x =_x;
        y =_y;
 
    }

	public float x; 
    public float y;
};
