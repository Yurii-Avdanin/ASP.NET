import { Component } from "react";
import './Styles.css';

interface PanelDataProps {
  stringArray: string[];
}  
  
export class PanelData extends Component<PanelDataProps, {}>
{
  render() {
    const { stringArray } = this.props;

    return (
      <div className="panel-green">
        {stringArray.map((item, index) => (
          <div key={index} className="panel-green-item">
            {item}
          </div>
        ))}
      </div>
    );
  }
}