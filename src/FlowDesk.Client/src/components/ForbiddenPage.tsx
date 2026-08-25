import forbiddenImg from '../assets/forbidden-mascot.png';

const ForbiddenPage = () => {
  return (
    <div style={styles.container}>
      <img 
        src={forbiddenImg} 
        alt="403 Forbidden Access" 
        style={styles.image} 
      />
      <h1 style={styles.title}>403 - Access Forbidden</h1>
      <p style={styles.subtitle}>
        You don't have permission to view this page.
      </p>
      <button 
        onClick={() => window.history.back()} 
        style={styles.button}
      >
        Go Back
      </button>
    </div>
  )
}

const styles: { [key: string]: React.CSSProperties } = {
  container: {
    minHeight: '100vh',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    textAlign: 'center',
    backgroundColor: '#0d1117',
    color: '#c9d1d9',
    padding: '24px',
    fontFamily: 'system-ui, -apple-system, sans-serif',
  },
  image: {
    maxWidth: '380px',
    width: '100%',
    height: 'auto',
    marginBottom: '24px',
  },
  title: {
    fontSize: '2rem',
    fontWeight: 700,
    margin: '0 0 8px 0',
    color: '#f0f6fc',
  },
  subtitle: {
    fontSize: '1rem',
    color: '#8b949e',
    margin: '0 0 24px 0',
  },
  button: {
    padding: '10px 20px',
    fontSize: '0.95rem',
    fontWeight: 600,
    color: '#ffffff',
    backgroundColor: '#238636',
    border: 'none',
    borderRadius: '6px',
    cursor: 'pointer',
    transition: 'background-color 0.2s ease',
  },
};

export default ForbiddenPage;