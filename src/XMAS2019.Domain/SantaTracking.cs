using System;

namespace XMAS2019.Domain
{
    public class SantaTracking
    {
        public SantaTracking(Guid id, GeoPoint canePosition, params Movement[] santaMovements)
        {
            Id = id;
            CanePosition = canePosition ?? throw new ArgumentNullException(nameof(canePosition));
            SantaMovements = santaMovements ?? throw new ArgumentNullException(nameof(santaMovements));
        }

        public Guid Id { get; }
        public GeoPoint CanePosition { get; }
        public Movement[] SantaMovements { get; }

        public GeoPoint CalculateSantaPositionAlternative(bool flipXAndY = false)
        {
            GeoPoint position = CanePosition;

            foreach (Movement movement in SantaMovements)
            {
                double meters = movement.Unit.ToMeters(movement.Value);

                double x = 0;
                double y = 0;

                switch (movement.Direction)
                {
                    case Direction.Up:
                        y = meters;
                        break;
                    case Direction.Right:
                        x = meters;
                        break;
                    case Direction.Down:
                        y = -1 * meters;
                        break;
                    case Direction.Left:
                        x = -1 * meters;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                position = flipXAndY
                    ? position.MoveTo(y, x)
                    : position.MoveTo(x, y);
            }

            return position;
        }

        public GeoPoint CalculateSantaPosition(bool flipXAndY = false)
        {
            double x = 0d;
            double y = 0d;

            foreach (Movement movement in SantaMovements)
            {
                double meters = movement.Unit.ToMeters(movement.Value);

                switch (movement.Direction)
                {
                    case Direction.Up:
                        y += meters;
                        break;
                    case Direction.Right:
                        x += meters;
                        break;
                    case Direction.Down:
                        y -= meters;
                        break;
                    case Direction.Left:
                        x -= meters;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return flipXAndY
                ? CanePosition.MoveTo(y, x)
                : CanePosition.MoveTo(x, y);
        }
    }
}