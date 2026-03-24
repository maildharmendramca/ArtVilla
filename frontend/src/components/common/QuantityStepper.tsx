interface QuantityStepperProps {
  value: number;
  onChange: (value: number) => void;
  min?: number;
  max?: number;
}

export default function QuantityStepper({ value, onChange, min = 1, max = 99 }: QuantityStepperProps) {
  return (
    <div className="flex items-center border border-gray-300 rounded-lg overflow-hidden">
      <button
        onClick={() => onChange(Math.max(min, value - 1))}
        disabled={value <= min}
        className="px-3 py-2 text-muted hover:bg-gray-100 disabled:opacity-40 transition-colors"
      >
        -
      </button>
      <span className="px-4 py-2 text-sm font-medium text-dark min-w-[3rem] text-center">{value}</span>
      <button
        onClick={() => onChange(Math.min(max, value + 1))}
        disabled={value >= max}
        className="px-3 py-2 text-muted hover:bg-gray-100 disabled:opacity-40 transition-colors"
      >
        +
      </button>
    </div>
  );
}
