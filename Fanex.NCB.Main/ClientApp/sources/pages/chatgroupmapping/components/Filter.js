import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { InputField } from '../../common/InputField';
import { SelectField } from '../../common/SelectField';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { MultipleSelectField } from '../../common/MultipleSelectField';

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
        console.log(this.props.settingData);

        return (
            <Form className="form-filter">
                <FormGroup>
                    <Label className="form-label">Bot Name</Label>
                    <MultipleSelectField
                        id={"botNames"}
                        width={200}
                        initialValue={this.state.botName.init}
                        data={this.props.settingData.botNames}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        noneSelectedLabel={"All"}
                        onChange={(value) => this.onChange("botNames", value)}
                    />       
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient Type</Label>
                    <MultipleSelectField
                        id={"recipientTypes"}
                        width={200}
                        initialValue={this.state.recipientType.init}
                        data={this.props.settingData.recipientTypes}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        noneSelectedLabel={"All"}
                        onChange={(value) => this.onChange("recipientTypes", value)}
                    />       
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Recipient</Label>
                    <InputField initialValue={this.state.recipient.init}
                        isSummited={this.state.isSummited}
                        onChange={(value) => this.onChange("recipient", value)} /> 
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">Mapping Status</Label>
                    <MultipleSelectField
                        id={"mappingStatuses"}
                        width={200}
                        initialValue={this.state.mappingStatus.init}
                        data={this.props.settingData.mappingStatuses}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        noneSelectedLabel={"All"}
                        onChange={(value) => this.onChange("mappingStatuses", value)}
                        />
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