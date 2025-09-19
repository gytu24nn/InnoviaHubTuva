import ".//errorboundry.css";

type ErrorBoundaryFallbackProps = {
  error: Error;
  resetErrorBoundary: () => void;
};


function ErrorBoundaryFallback({ error, resetErrorBoundary}: ErrorBoundaryFallbackProps) {
    console.log("Errorboundary caught an error: ", error);
    
    return (
        <div className="errorBoundaryWrapper" role="alert">
            <div className="rotatingGear large"></div>
            <div className="rotatingGear small"></div>

            <div className="errorCard">
                <h1 className="errorTitle">Oops! An error occurred</h1>
                <p className="errorMessage">{error.message}</p>
                <button className="errorButton" onClick={resetErrorBoundary}>
                    Try again!
                </button>
            </div>
            
            
        </div>
    )
}

export default ErrorBoundaryFallback;