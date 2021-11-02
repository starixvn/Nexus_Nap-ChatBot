import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
import PanelContent from '../../common/panel-content/PanelContent';
import ContentPreview from '../../common/ContentPreview';
var dateFormat = require("dateformat");

export class DataGridConversationMessage extends Component {
    constructor(props) {
        super(props);

        this.state = {
            expandPanels: false
        };

        this.panels = [];
    }

    toggleExpandPanels = () => {
        this.setState(prevState => ({ expandPanels: !prevState.expandPanels }), () => {
            if (this.props.conversationMessages.total > 0) {
                this.panels.filter(x => x && (x.state.isOpen !== this.state.expandPanels)).forEach(panel => {
                    panel.handleSet(this.state.expandPanels);
                });
            }
        });
    };

    buildHeader = () => {
        return (
            <Row >
                <Cell colWidth={15} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={20} header>{Labels.BotName}</Cell>
                <Cell colWidth={16} header>{Labels.ConversationGroupID}</Cell>
                <Cell colWidth={40} header>{Labels.ConversationGroupName}</Cell>
                <Cell colWidth={20} header>{Labels.Recipient}</Cell>
                <Cell colWidth={16} header>{`${Labels.SentTime} (GMT+8)`}</Cell>
                <Cell colWidth={55} header>
                    <div className="has-icons">
                        {Labels.Content}
                        <span className={this.state.expandPanels ? 'icon-collapse' : 'icon-expand'} onClick={this.toggleExpandPanels} />
                    </div>
                </Cell>
                <Cell colWidth={16} header>{Labels.IsSuccessful}</Cell>
                <Cell colWidth={45} header>{Labels.ErrorMessage}</Cell>
            </Row>
        );
    };

    buildBody = () => {
        var conversationData = this.props.conversationMessages.data;
        var conversationDataRecipient = this.props.conversationMessages.dataMapRecipient;

        if (this.props.conversationMessages.total > 0) {
            const newData = conversationData.map((element,index) => {
                var item = conversationDataRecipient.filter((item) => item.conversationID === element.toConversationID);
                if(item.length){
                    return {...element,"recipientName": item[0].recipientName};
                }
                return element;
            });
            
            return newData.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="10"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index) => (
        <Row key={index} >
            <Cell className="text-center">{item.notificationID}</Cell>
            <Cell className="text-center">{item.botName}</Cell>
            <Cell className="text-center">{item.conversationGroupID}</Cell>
            <Cell>{item.conversationGroupName}</Cell>
            <Cell className="text-center">{item.recipientName}</Cell>
            <Cell className="text-center">{dateFormat(item.sentTime, DateTimeFormatV2)}</Cell>
            <Cell>
                {
                    (item.content && item.content.length > 132) ? <PanelContent ref={panel => this.panels[index] = panel} content={item.content} />
                        : <ContentPreview text={item.content} />
                }
            </Cell>
            <Cell className="text-center">{item.isSuccessful}</Cell>
            <Cell>{item.errorMessage}</Cell>
        </Row>
    );

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getConversationMessageLog(params);
        this.props.getParamsPageConversationMessage(params);
    };

    render() {
        const pagingOption = {
            PageIndex: this.props.paramsPageConversationMessage.pageIndex,
            PageSize: this.props.paramsPageConversationMessage.pageSize,
            TotalItem: this.props.conversationMessages.total ? this.props.conversationMessages.total : 0,
            PageList: [100, 200, 300, 500, 1000]
        };

        return (
            <Table
                width={1620}
                autoWidth={false}
                header={this.buildHeader()}
                body={this.buildBody()}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={285}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPageConversationMessage: state.paramsPageConversationMessage,
        conversationMessages: state.conversationMessages
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridConversationMessage);
