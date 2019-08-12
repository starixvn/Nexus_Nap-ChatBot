import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import CommonFunctions from '../../common/CommonFunctions';
import { PAGING_SIZES, mappingStatus } from '../../constants/constants';
import Labels from '../../common/Labels';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';

export class DataGrid extends Component {
    constructor(props) {
        super(props);
    }

    onPaging = (pageIndex, pageSize) => {
        const params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    openEditPopup = item => {
        if (!CommonFunctions.isBotRemoved(item.mappingStatusID)) {
            this.props.filterRecipients(this.props.settingData.recipients, item.recipientTypeID);
            this.props.openEditPopup(item);
        }
    }

    buildHeader = (isHost) => {
        return (
            <Row style={{ height: 32 }}>
                <Cell colWidth={30} header>{Labels.NumberCol}</Cell>
                <Cell colWidth={100} header>{Labels.BotName}</Cell>
                <Cell colWidth={300} header>{Labels.GroupID}</Cell>
                <Cell colWidth={200} header>{Labels.GroupName}</Cell>
                <Cell colWidth={110} header>{Labels.ChatGroupType}</Cell>
                <Cell colWidth={100} header>{Labels.RecipientType}</Cell>
                <Cell colWidth={130} header>{Labels.Recipient}</Cell>
                <Cell colWidth={100} header>{Labels.LicenseeStatus}</Cell>
                <Cell colWidth={100} header>{Labels.MappingStatus}</Cell>
                <Cell colWidth={100} header>{Labels.Remark}</Cell>
                {
                    isHost &&
                    <Cell colWidth={60} header>{Labels.Action}</Cell>
                }
            </Row>);
    };

    copyGroupId = (text) => {
        var input = document.createElement('textarea', value = text);
        input.select();
        document.execCommand("copy");
        console.log(input.value);
    }

    getLicenseeStatus = item => item.recipientTypeName ? CommonFunctions.getLicenseeStatus(item.recipientStatusNames) : '';

    getEditClassNames = item => CommonFunctions.isBotRemoved(item.mappingStatusID) ? 'icon-edit edit-disabled' : 'icon-edit';

    buildBody = (data, pageIndex, pageSize, isHost) => {
        const rows = [];
        const startIndex = (pageIndex - 1) * pageSize + 1;
        if (data.length > 0) {
            data.map((item, index) => {
                rows.push(<Row key={`body${index}`} >
                    <Cell className="text-center">{index + startIndex}</Cell>
                    <Cell>{item.mappingStatusID === mappingStatus.BotRemoved ? '' : item.botName}</Cell>
                    <Cell title={item.conversationID}>
                        <span
                            onClick={() => this.copyGroupId(item.conversationID)}>
                            {item.conversationID}</span>
                    </Cell>
                    <Cell>{item.groupName}</Cell>
                    <Cell>{item.chatGroupTypeName}</Cell>
                    <Cell>{item.recipientTypeName}</Cell>
                    <Cell>{item.recipientNames}</Cell>
                    <Cell>{this.getLicenseeStatus(item)}</Cell>
                    <Cell><span className={CommonFunctions.getMappingClass(item.mappingStatusID)}>{item.mappingStatusName}</span></Cell>
                    <Cell>{item.remark}</Cell>
                    {
                        isHost &&
                        <Cell className="text-center col-action">
                            <div>
                                <span className={this.getEditClassNames(item)} id={'edit-' + index} onClick={() => this.openEditPopup(item)} />
                            </div>
                        </Cell>
                    }
                </Row>);
            });
        } else {
            rows.push(<Row key={`body${1}`} ><Cell colSpan="10"><NoDataTitle /></Cell></Row >);
        }

        return rows;
    }

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const { data, count, isHost } = this.props.gridData;
        const { pageIndex, pageSize } = this.props.gridParams;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: count,
            PageList: PAGING_SIZES
        };

        return (
            <div className="table-container-div">
                <Table
                    width={1620}
                    autoWidth={false}
                    header={this.buildHeader(isHost)}
                    body={this.buildBody(data, pageIndex, pageSize, isHost)}
                    isPaging
                    pageOption={pagingOption}
                    onPaging={this.onPaging}
                    containerPadding={0}
                />
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, gridData, settingData } = state;
    return {
        gridParams,
        gridData,
        settingData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);