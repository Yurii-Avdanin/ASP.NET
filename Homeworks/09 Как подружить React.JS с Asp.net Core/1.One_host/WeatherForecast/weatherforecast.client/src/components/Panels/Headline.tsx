import { Container} from 'react-bootstrap';
import { Card } from 'react-bootstrap';
import './_Styles.css';

const Headline = ({ text }: { text: string }) => {
    return (
      <Container>
        <Card className='CardTitle'>
          <Card.Body>
            <Card.Title as="h2" className="text-center mb-4">
              {text}
            </Card.Title>
          </Card.Body>
        </Card>
      </Container>
    );
};
    
export default Headline;
