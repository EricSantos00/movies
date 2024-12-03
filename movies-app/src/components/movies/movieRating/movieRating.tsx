import { StarIcon } from "@/components/common/icons/starIcon";

type MovieRatingProps = {
    rating: number;
};

export function MovieRating({ rating }: MovieRatingProps) {
    const maxRating = 5;
    const totalStars = 5;
    const normalizedRating = (rating / maxRating) * totalStars;
    const filledStars = Math.floor(normalizedRating);
    const hasHalfStar = normalizedRating % 1 >= 0.5;
    const emptyStars = totalStars - filledStars - (hasHalfStar ? 1 : 0);

    return (
        <div className="flex items-center space-x-2">
            <div className="flex">
                {Array.from({ length: filledStars }).map((_, idx) => (
                    <StarIcon key={`filled-${idx}`} type="filled" />
                ))}
                {hasHalfStar && <StarIcon type="half" />}
                {Array.from({ length: emptyStars }).map((_, idx) => (
                    <StarIcon key={`empty-${idx}`} type="empty" />
                ))}
            </div>
            <span className="text-sm font-medium text-gray-700">
                {rating}/{maxRating}
            </span>
        </div>
    );
}