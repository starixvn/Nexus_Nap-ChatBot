import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';

class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            botName: {
                init: 1,
                data: []
            },
            recipientType: {
                init: 1,
                data: []
            },
            recipient: {
                init: "",
                data: []
            },
            mappingStatus: {
                init: 1,
                data: []
            }
        };
    }

    componentWillMount() {
        this.props.getSettings();
    }  

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    submitHandler = () => {
        e.preventDefault();
    }
  
    render() {
        return (
            <Form className="form-filter">
                <FormGroup>
                    <Label className="form-label">Bot Name</Label>
                    <SelectField initialValue={this.state.botName.init}
                        data={this.state.botName.data}
                        isSummited={this.state.isSummited}
                        onChange={(value) => this.onChange("botName", value)} />    
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient Type</Label>
                    <SelectField initialValue={this.state.recipientType.init}
                        data={this.state.recipientType.data}
                        isSummited={this.state.isSummited}
                        onChange={(value) => this.onChange("recipientType", value)} />  
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient</Label>
                    <InputField initialValue={this.state.recipient.init}
                        isSummited={this.state.isSummited}
                        onChange={(value) => this.onChange("recipient", value)} /> 
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Mapping Status</Label>
                    <SelectField initialValue={this.state.mappingStatus.init}
                        data={this.state.mappingStatus.data}
                        isSummited={this.state.isSummited}
                        onChange={(value) => this.onChange("mappingStatus", value)} />  
                </FormGroup>
                <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
            </Form>            
        );
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { settingData} = state;
    return Object.assign({}, { settingData });
}

export default connect(mapStateToProps, mapDispatchToProps)(Filter);