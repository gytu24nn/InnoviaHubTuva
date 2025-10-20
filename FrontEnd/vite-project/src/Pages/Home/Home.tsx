import "./Home.css"
import { Link, useNavigate } from "react-router-dom"
import SensorSummary from "../../Components/SensorSummary";

const Home = () => {
  const navigate = useNavigate();
  
  //Test for errorbondary
  //throw new Error("Testfel från HomePage");

  const handleGettingStartedButton = () => {
    navigate("/Resursvy");
  }
  return (
    <div className="HomePage">
      
      <div className="HeroImage">
        <img src="img/HomePicture.jpg" alt="Kontosmiljö" />
      </div>
      
      <div className="HeroSection">
        <h1 className="WelcomeText">Välkommen till Innovia Hub</h1>

        <p className="InfoText">
          Ett dynamiskt coworking- och forskningscenter där
          företag, startups och universitet kan använda 
          vår plattform för att boka resurser. 
          Oavsett om du behöver ett mötesrum, 
          en arbetsstation eller specialutrustning. 

          så kan du enkelt boka resurser och hantera dina bokningar.
        </p>
      </div>

      <div className="SmartTips-section">
        <h3 className="SmartTipsText">Upptäck våra SmartTips!</h3>
        <p className="InfoText">
          Med SmartTips får du hjälp i bokningssystemet att hitta de lugnaste dagarna i veckan – perfekt innan du valt datum.
          När du har valt ett datum och en specifik resurs får du AI-genererade tips om hur dagen ser ut, så att du kan boka smartare och enklare. ✨🤖
        </p>
      </div>
      
      <SensorSummary/>

      <div className="Icons">

        <div className="IconBox">
          <i className="fa-solid fa-calendar-check"></i>
          <h3>Boka resurser</h3>
        </div>

        <div className="IconBox">
          <i className="fa-solid fa-list-check"></i>
          <h3>Mina bokningar</h3>
        </div>

        <div className="IconBox">
          <i className="fa-solid fa-robot"></i>
          <h3>SmartTips</h3>

        </div>
      </div>

      <button className="GettingStartedButton" onClick={handleGettingStartedButton}>
        Boka nu
      </button>

      

    </div>
  )
}

export default Home