import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Form, FormGroup, Label } from 'reactstrap';
import { InputField } from '../../common/InputField';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { MultipleSelectField } from '../../common/MultipleSelectField';
import Labels from '../../common/Labels';
import AutoComplete from '../../common/auto-complete';
import Site from '../../lib/site';
import { PAGING_SIZES } from '../../constants/constants';

export class Filter extends Component {
    constructor(props) {
        super(props);

        this.state = {
            botNames: {
                value: ""
            },
            recipientTypes: {
                value: ""
            },
            recipient: {
                value: ""
            },
            licenseeStatuses: {
                value: ""
            },
            mappingStatuses: {
                value: ""
            },
            settingData: {
                botNames: [],
                recipientTypes: [],
                mappingStatuses: [],
                licenseeStatuses: []
            }
        };

        this.props.getSettings();
        this.props.filterRecipients([], 0);
    }

    static getDerivedStateFromProps(nextProps, prevState) {       
        console.log("getDerivedStateFromProps");
        if (prevState.settingData.botNames.length === 0 && nextProps.settingData.botNames) {
            let botNames = nextProps.settingData.botNames;
            let selectedBotNames = [];
            botNames.map((v) => {
                v['checked'] = true;
                selectedBotNames.push(v['id']);
            });

            let recipientTypes = nextProps.settingData.filterRecipientTypes;
            let selectedRecipientTypes = [];
            recipientTypes.map((v) => {
                v['checked'] = true;
                selectedRecipientTypes.push(v['id']);
            });

            let mappingStatuses = nextProps.settingData.mappingStatuses;
            let selectedMappingStatuses = [];
            mappingStatuses.map((v) => {
                v['checked'] = true;
                selectedMappingStatuses.push(v['id']);
            });

            let licenseeStatuses = nextProps.settingData.licenseeStatuses;
            let selectedlicenseeStatuses= [];
            licenseeStatuses.map((v) => {
                v['checked'] = true;
                selectedlicenseeStatuses.push(v['id']);
            });

            let state = {
                botNames: {
                    value: selectedBotNames.join(",")
                },
                recipientTypes: {
                    value: selectedRecipientTypes.join(",")
                },
                recipient: {
                    value: ""
                },
                licenseeStatuses: {
                    value: selectedlicenseeStatuses.join(",")
                },
                mappingStatuses: {
                    value: selectedMappingStatuses.join(",")
                },
                settingData: {
                    botNames: botNames,
                    recipientTypes: recipientTypes,
                    mappingStatuses: mappingStatuses,
                    licenseeStatuses: licenseeStatuses
                }
            };

            return state;
        }

        // Return null to indicate no change to state.
        return null;
    }  

    onChange = (fieldName, fieldValue) => {
        this.setState({
            [fieldName]: fieldValue
        });
    }

    getParams = () => {           
        return {
            botNames: this.state.botNames.value,
            recipientTypeIds: this.state.recipientTypes.value,
            recipientName: this.state.recipient.value,
            mappingStatusIds: this.state.mappingStatuses.value,
            licenseeStatusIds: this.state.licenseeStatuses.value,
            pageIndex: 1,
            pageSize: PAGING_SIZES[0]
        };
    }    

    checkValidParams = () => {
        return this.state.botNames.value !== '' && this.state.recipientTypes.value !== '' && this.state.mappingStatuses.value !== '' && this.state.licenseeStatuses.value !== '';
    }

    submitHandler = (e) => {
        e.preventDefault();

        let params = this.getParams();
        this.props.setGridParams(params);
        this.props.getGridData(params);
    }   
  
    render() {        
        return (
            <Form className="form-filter">                
                <FormGroup>
                    <Label className="form-label">{Labels.RecipientType}</Label>
                    <MultipleSelectField
                        id={"recipientTypes"}
                        width={200}
                        initialValue={this.state.recipientTypes}
                        data={this.state.settingData.recipientTypes}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: "# selected" }}
                        isAllTextShown
                        onChange={(value) => this.onChange("recipientTypes", value)}
                    />       
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.Recipient}</Label>
                    <AutoComplete
                        url={Site.resolveClientUrl("ChatGroupMapping/SearchRecipient")}
                        onSelect={(value) => this.onChange("recipient", { value: value })}
                        onInput={(value) => this.onChange("recipient", { value: value })}
                        htmlAttributes={{ maxLength: 50 }}
                    /> 
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.LicenseeStatus}</Label>
                    <MultipleSelectField
                        id={"licenseeStatuses"}
                        width={200}
                        initialValue={this.state.licenseeStatuses}
                        data={this.state.settingData.licenseeStatuses}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: "# selected" }}
                        isAllTextShown
                        onChange={(value) => this.onChange("licenseeStatuses", value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.MappingStatus}</Label>
                    <MultipleSelectField
                        id={"mappingStatuses"}
                        width={200}
                        initialValue={this.state.mappingStatuses}
                        data={this.state.settingData.mappingStatuses}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: "# selected" }}
                        isAllTextShown
                        onChange={(value) => this.onChange("mappingStatuses", value)}
                    />
                </FormGroup>
                <FormGroup>
                    <Label className="form-label">{Labels.BotName}</Label>
                    <MultipleSelectField
                        id={"botNames"}
                        width={200}
                        initialValue={this.state.botNames}
                        data={this.state.settingData.botNames}
                        keyField={"id"}
                        valueField={"name"}
                        statusField={"checked"}
                        isSummited={false}
                        texts={{ SelectOptions: Labels.PleaseSelect, All: Labels.All, SelectedItemCount: "# selected" }}
                        isAllTextShown
                        onChange={(value) => this.onChange("botNames", value)}
                    />       
                </FormGroup>
                <button disabled={!this.checkValidParams()} className="btn-submit" onClick={this.submitHandler}>{Labels.Submit}</button>
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