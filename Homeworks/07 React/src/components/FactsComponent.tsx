import { Component } from "react";
import { PanelError } from './PanelError';
import { PanelData } from './PanelData';
import './Styles.css';

interface FactsState {
    isLoading: boolean;
    facts: string[];
    error: string | null;
  }

export class FactsComponent extends Component<{}, FactsState> 
{
    constructor(props: {}) {
        super(props);
        this.state = { 
            isLoading: false, 
            facts: [],  
            error: null 
        }        
    } 
    
    fetchFact = async () => {    
        try {
            this.setState({ isLoading: true, error: null });

            const response = await fetch('https://catfact.ninja/facts?limit=15');
            
            //-- Для проверки ответа, отличного от 2**
            //const response = await fetch('https://httpbin.org/status/403');
      
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const data = await response.json();           
            
            const factStrings = data.data.map((item: any) => item.fact);

            this.setState({ facts: factStrings });
        } 
        catch (err) {
            this.setState({ error: err instanceof Error ? err.message : 'Неизвестная ошибка' });
        }
        finally {
            this.setState({ isLoading: false })            
        }
    }

    render() {
        const { isLoading, facts, error } = this.state;
        
        return (
            <div className="ramka">
              <button className="button-api" onClick={this.fetchFact} disabled={isLoading}>
                {isLoading ? 'Загрузка...' : 'Получить факты о котах'}
              </button>      
              { error && <PanelError message={error}/> }
              { facts.length > 0 && <PanelData stringArray={facts} />}
            </div>
        );
    }
}