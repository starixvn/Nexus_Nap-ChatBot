import newNotification, {
    setNotificationTypes,
    setNotificationTemplates,
    setProducts,
    setRecipients,
    setPlatforms,
    setConfirmationPopup,
    setResetConfirmationPopup,
    setEditConfirmationPopup,
    setMarkdownRulesPopup,
    setCreateNotificationResult
} from '../newNotificationSlice';

describe('newNotificationSlice reducer', () => {
    it('should handle initial state', () => {
        expect(newNotification(undefined, {})).toEqual({
            notificationTypes: [],
            notificationTemplates: [],
            products: [],
            recipients: [],
            platforms: [],
            confirmationPopup: {
                isOpen: false,
                content: ''
            },
            resetConfirmationPopup: {
                isOpen: false
            },
            editConfirmationPopup: {
                isOpen: false
            },
            markdownRulesPopup: {
                isOpen: false
            },
            createNotificationResult: null
        });
    });

    it('should handle setNotificationTypes', () => {
        expect(
            newNotification({}, {
                type: setNotificationTypes.type,
                payload: [{ id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 }]
            })
        ).toEqual({
            notificationTypes: [{ id: 1, name: 'Special Delete', checked: true, newMaxMessageId: 0 }]
        });
    });

    it('should handle setNotificationTemplates', () => {
        expect(
            newNotification({}, {
                type: setNotificationTemplates.type,
                payload: [{ id: 1, name: 'test' }]
            })
        ).toEqual({
            notificationTemplates: [{ id: 1, name: 'test' }]
        });
    });

    it('should handle setProducts', () => {
        expect(
            newNotification({}, {
                type: setProducts.type,
                payload: [{ id: 1, name: 'test' }]
            })
        ).toEqual({
            products: [{ id: 1, name: 'test' }]
        });
    });

    it('should handle setRecipients', () => {
        expect(
            newNotification({}, {
                type: setRecipients.type,
                payload: [{ id: 1, name: 'test' }]
            })
        ).toEqual({
            recipients: [{ id: 1, name: 'test' }]
        });
    });

    it('should handle setPlatforms', () => {
        expect(
            newNotification({}, {
                type: setPlatforms.type,
                payload: [{ platformId: 1, platformName: 'test' }]
            })
        ).toEqual({
            platforms: [{ platformId: 1, platformName: 'test' }]
        });
    });

    it('should handle setConfirmationPopup', () => {
        expect(
            newNotification({}, {
                type: setConfirmationPopup.type,
                payload: [{ isOpen: true, content: 'test' }]
            })
        ).toEqual({
            confirmationPopup: [{ isOpen: true, content: 'test' }]
        });
    });

    it('should handle setResetConfirmationPopup', () => {
        expect(
            newNotification({}, {
                type: setResetConfirmationPopup.type,
                payload: [{ isOpen: true, content: 'test' }]
            })
        ).toEqual({
            resetConfirmationPopup: [{ isOpen: true, content: 'test' }]
        });
    });

    it('should handle setEditConfirmationPopup', () => {
        expect(
            newNotification({}, {
                type: setEditConfirmationPopup.type,
                payload: [{ isOpen: true, content: 'test' }]
            })
        ).toEqual({
            editConfirmationPopup: [{ isOpen: true, content: 'test' }]
        });
    });

    it('should handle setMarkdownRulesPopup', () => {
        expect(
            newNotification({}, {
                type: setMarkdownRulesPopup.type,
                payload: [{ isOpen: true, content: 'test' }]
            })
        ).toEqual({
            markdownRulesPopup: [{ isOpen: true, content: 'test' }]
        });
    });

    it('should handle setCreateNotificationResult', () => {
        expect(
            newNotification({}, {
                type: setCreateNotificationResult.type,
                payload: [{ isSuccessful: true, message: 'success' }]
            })
        ).toEqual({
            createNotificationResult: [{ isSuccessful: true, message: 'success' }]
        });
    });
});