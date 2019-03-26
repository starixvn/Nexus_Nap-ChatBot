import React, { Component } from 'react';
import { Input, FormFeedback } from 'reactstrap';
import './SelectField.scss';

export class SelectField extends Component {
  constructor(props) {
    super(props);

    this.state = {
        value: props.initialValue
    };
  }  

  onChange = (e) => {    
    this.setState(
        {
            value: e.target.value
        },
        () => {
            this.props.onChange({
                value: this.state.value,
                isValid: this.isValid()
            });
        }
    );
  }  

  isValid = () => {
    return this.state.value !== -1;
  }

  render() {   
      return (
        <div className="select-field">
            <Input 
                type="select"
                value={this.state.value}
                onChange={this.onChange}
                invalid={this.props.isSummited && !this.isValid()}
            >
                <option value="-1">---Please select---</option>
                {                            
                    this.props.data &&
                        this.props.data.map((item, index) => 
                            <option key={index} value={item.id} >{item.name}</option>
                        )                            
                }
            </Input>
            { this.props.isSummited && !this.isValid() &&  <FormFeedback>This field is required</FormFeedback> }  
      </div>
    );
  }
}
