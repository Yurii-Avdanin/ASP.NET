import { Component } from "react";
import './Styles.css';

interface PanelErrorProps {
    message: string;
}

export class PanelError extends Component<PanelErrorProps, {}>
{
  render() {
    const { message } = this.props;

    return (
      <div className="panel-red">
         {message}
      </div>
    );
  }
}
