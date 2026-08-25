interface IconProps {
  className?: string;
}

const FlowDeskIcon: React.FC<IconProps> = ({ className = 'h-7 w-7' }) => {
  return (
    <svg
      viewBox="0 0 32 32"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      className={className}
      aria-hidden="true"
    >
      <rect width="32" height="32" rx="8" className="fill-accent" />
      {/* Top flow stroke */}
      <path
        d="M9 10C9 8.89543 9.89543 8 11 8H22C22.5523 8 23 8.44772 23 9C23 9.55228 22.5523 10 22 10H12C11.4477 10 11 10.4477 11 11V22C11 22.5523 10.5523 23 10 23C9.44772 23 9 22.5523 9 22V10Z"
        fill="white"
      />
      {/* Mid tier stroke */}
      <path
        d="M14 15C14 14.4477 14.4477 14 15 14H20C20.5523 14 21 14.4477 21 15C21 15.5523 20.5523 16 20 16H16V20C16 20.5523 15.5523 21 15 21C14.4477 21 14 20.5523 14 20V15Z"
        fill="white"
        fillOpacity="0.85"
      />
      {/* Dynamic accent dot */}
      <circle cx="20.5" cy="20.5" r="2.5" fill="white" />
    </svg>
  )
}

export default FlowDeskIcon;